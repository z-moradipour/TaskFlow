import React, { useState, useEffect } from 'react';
import Modal from 'react-modal';

const customStyles = {
  content: {
    top: '50%', left: '50%', right: 'auto', bottom: 'auto',
    marginRight: '-50%', transform: 'translate(-50%, -50%)',
    width: '500px',
    padding: '20px'
  },
  overlay: {
    backgroundColor: 'rgba(0, 0, 0, 0.75)'
  }
};

Modal.setAppElement('#root');

function CardModal({ isOpen, onRequestClose, card, onSave, onDelete }) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  useEffect(() => {
    if (card) {
      setTitle(card.title);
      setDescription(card.description || '');
    }
  }, [card]);

  const handleSave = () => {
    onSave({ ...card, title, description });
  };
  
  const handleDelete = () => {
    if (window.confirm(`Are you sure you want to delete the card "${card.title}"?`)) {
        onDelete(card.id);
    }
  };

  if (!card) return null;

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={onRequestClose}
      style={customStyles}
      contentLabel="Edit Card"
    >
      <div className="card-modal">
        <input 
          type="text" 
          value={title} 
          onChange={(e) => setTitle(e.target.value)} 
          className="card-modal-title-input"
        />
        <p className="card-modal-section-title">Description</p>
        <textarea 
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Add a more detailed description..."
          className="card-modal-desc-textarea"
        ></textarea>
        <div className="card-modal-actions">
            <button onClick={handleSave} className="card-modal-save-btn">Save</button>
            {}
            <button onClick={handleDelete} className="card-modal-delete-btn">Delete Card</button>
        </div>
      </div>
    </Modal>
  );
}

export default CardModal;