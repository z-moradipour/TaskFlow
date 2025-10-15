import { useState } from 'react';
import api from '../api/axiosConfig.js';
import { useDroppable } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { DraggableCard } from './DraggableCard.jsx';

function ListComponent({ list, onDeleteList, setLists, onCardClick }) {
  const [newCardTitle, setNewCardTitle] = useState('');
  const { setNodeRef } = useDroppable({ id: `list-${list.id}` });
  
  const [isEditingTitle, setIsEditingTitle] = useState(false);
  const [editedTitle, setEditedTitle] = useState(list.title);

const handleCreateCard = async (e) => {
  e.preventDefault();
  if (!newCardTitle.trim()) return;

  try {
      const newCardDto = { title: newCardTitle };
      const response = await api.post(`/Cards/list/${list.id}`, newCardDto);
      setLists(prevLists =>
          prevLists.map(l =>
              l.id === list.id
                  ? { ...l, cards: [...l.cards, response.data] }
                  : l
          )
      );
      setNewCardTitle('');
  } catch (err) {
      console.error("Error creating card:", err);
  }
};

  const handleTitleSave = async () => {
    if (!editedTitle.trim() || editedTitle === list.title) {
      setIsEditingTitle(false);
      setEditedTitle(list.title);
      return;
    }

    try {
      const updateDto = { title: editedTitle };
      await api.put(`https://localhost:7289/api/Lists/${list.id}`, updateDto);
      
      setLists(prevLists => 
        prevLists.map(l => 
          l.id === list.id ? { ...l, title: editedTitle } : l
        )
      );
      setIsEditingTitle(false);
    } catch (error) {
      console.error("Failed to update list title:", error);
      setEditedTitle(list.title);
      setIsEditingTitle(false);
    }
  };

  const handleTitleKeyDown = (e) => {
    if (e.key === 'Enter') {
      handleTitleSave();
    } else if (e.key === 'Escape') {
      setEditedTitle(list.title);
      setIsEditingTitle(false);
    }
  };

  return (
    <div ref={setNodeRef} className="list">
      <div className="list-header">
        {isEditingTitle ? (
          <input
            type="text"
            value={editedTitle}
            onChange={(e) => setEditedTitle(e.target.value)}
            onBlur={handleTitleSave}
            onKeyDown={handleTitleKeyDown}
            autoFocus
            className="list-title-input"
          />
        ) : (
          <h3 onClick={() => setIsEditingTitle(true)}>{list.title}</h3>
        )}
        <button onClick={() => onDeleteList(list.id)} className="delete-button">×</button>
      </div>
      <div className="cards-container">
        <SortableContext 
          items={list.cards.map(c => `card-${c.id}`)}
          strategy={verticalListSortingStrategy}
        >
          {list.cards.map(card => (
            <DraggableCard 
                key={card.id} 
                card={card} 
                onCardClick={onCardClick}
            />
          ))}
        </SortableContext>
      </div>
      <form onSubmit={handleCreateCard} className="add-card-form">
        <input
          type="text"
          value={newCardTitle}
          onChange={(e) => setNewCardTitle(e.target.value)}
          placeholder="Enter card title..."
        />
        <button type="submit">Add Card</button>
      </form>
    </div>
  );
}

export default ListComponent;