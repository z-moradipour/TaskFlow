import React, { useState, useEffect } from 'react';
import Modal from 'react-modal';
import api from '../api/axiosConfig.js';

const customStyles = {
    content: {
      top: '50%',
      left: '50%',
      right: 'auto',
      bottom: 'auto',
      marginRight: '-50%',
      transform: 'translate(-50%, -50%)',
      width: '400px',
      padding: '20px'
    },
    overlay: {
      backgroundColor: 'rgba(0, 0, 0, 0.75)'
    }
  };
  
  Modal.setAppElement('#root');

function ShareBoardModal({ isOpen, onRequestClose, boardId, currentUserRole }) {
  const [username, setUsername] = useState('');
  const [message, setMessage] = useState('');
  const [isError, setIsError] = useState(false);
  const [members, setMembers] = useState([]); // State to hold the list of members

  // Fetch members when the modal opens
  useEffect(() => {
    if (isOpen) {
      const fetchMembers = async () => {
        try {
          const response = await api.get(`/boards/${boardId}/members`);
          setMembers(response.data);
        } catch (error) {
          console.error("Failed to fetch members:", error);
        }
      };
      fetchMembers();
    }
  }, [isOpen, boardId]);

  const handleInvite = async (e) => {
    e.preventDefault();
    if (!username.trim()) return;
    setMessage('');
    setIsError(false);

    try {
      const response = await api.post(`/boards/${boardId}/members/invite`, { username });
      setMessage(response.data || 'User invited successfully!');
      setUsername('');
      // Refresh the members list after inviting a new one
      const updatedMembers = await api.get(`/boards/${boardId}/members`);
      setMembers(updatedMembers.data);
    } catch (error) {
      setIsError(true);
      setMessage(error.response?.data || 'An error occurred.');
    }
  };
  
  const handleRemoveMember = async (userId) => {
    if (window.confirm("Are you sure you want to remove this member from the board?")) {
      try {
        await api.delete(`/boards/${boardId}/members/${userId}`);
        // Remove the member from the local state for an instant UI update
        setMembers(members.filter(member => member.userId !== userId));
      } catch (error) {
        setIsError(true);
        setMessage(error.response?.data || 'Failed to remove member.');
        console.error("Failed to remove member:", error);
      }
    }
  };

  const handleClose = () => {
    setMessage('');
    setIsError(false);
    setUsername('');
    onRequestClose();
  };
  
  return (
    <Modal isOpen={isOpen} onRequestClose={handleClose} style={customStyles} contentLabel="Share Board">
      <div className="share-modal-content">
        <h2>Manage Board Members</h2>

        {/* --- Section to view and remove members --- */}
        <div className="members-list-section">
          <h4>Current Members</h4>
          <ul className="members-list">
            {members.map(member => (
              <li key={member.userId}>
                <span>{member.username} ({member.role})</span>
                {/* Only show the remove button if the current user is the owner AND the member is not the owner */}
                {currentUserRole === 'Owner' && member.role !== 'Owner' && (
                  <button onClick={() => handleRemoveMember(member.userId)} className="remove-member-button">Remove</button>
                )}
              </li>
            ))}
          </ul>
        </div>
        
        <hr />

        {/* --- Section to invite new members --- */}
        {currentUserRole === 'Owner' && (
          <form onSubmit={handleInvite}>
            <h4>Invite New User</h4>
            <p>Enter the username of the person you want to invite.</p>
            <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" className="share-modal-input" />
            <button type="submit" className="share-modal-button">Invite</button>
          </form>
        )}
        
        {message && <p className={isError ? 'error-message' : 'success-message'}>{message}</p>}
      </div>
    </Modal>
  );
}

export default ShareBoardModal;