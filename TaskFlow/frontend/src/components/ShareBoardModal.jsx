import React, { useState } from 'react';
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

function ShareBoardModal({ isOpen, onRequestClose, boardId }) {
  const [username, setUsername] = useState('');
  const [message, setMessage] = useState('');
  const [isError, setIsError] = useState(false);

  const handleInvite = async (e) => {
    e.preventDefault();
    if (!username.trim()) return;

    setMessage('');
    setIsError(false);

    try {
      const response = await api.post(`/boards/${boardId}/members/invite`, { username });
      setMessage(response.data || 'User invited successfully!');
      setUsername(''); // Clear input on success
    } catch (error) {
      setIsError(true);
      setMessage(error.response?.data || 'An error occurred.');
      console.error("Failed to invite user:", error);
    }
  };
  
  // Clear message when modal is closed
  const handleClose = () => {
    setMessage('');
    setIsError(false);
    setUsername('');
    onRequestClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={handleClose}
      style={customStyles}
      contentLabel="Share Board"
    >
      <div className="share-modal-content">
        <h2>Invite User to Board</h2>
        <form onSubmit={handleInvite}>
          <p>Enter the username of the person you want to invite.</p>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username"
            className="share-modal-input"
          />
          <button type="submit" className="share-modal-button">Invite</button>
        </form>
        {message && (
          <p className={isError ? 'error-message' : 'success-message'}>
            {message}
          </p>
        )}
      </div>
    </Modal>
  );
}

export default ShareBoardModal;