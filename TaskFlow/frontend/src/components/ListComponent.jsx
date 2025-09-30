import { useState } from 'react';
import axios from 'axios';
import { useDroppable } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { DraggableCard } from './DraggableCard.jsx';

function ListComponent({ list, onDeleteList, setLists, onCardClick }) {
  const [newCardTitle, setNewCardTitle] = useState('');
  const { setNodeRef } = useDroppable({ id: `list-${list.id}` });

  const handleCreateCard = async (e) => {
    e.preventDefault();
    if (!newCardTitle.trim()) return;

    try {
      const newCardDto = { title: newCardTitle };
      const response = await axios.post(`https://localhost:7289/api/Cards/list/${list.id}`, newCardDto);
      
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

  return (
    <div ref={setNodeRef} className="list">
      <div className="list-header">
        <h3>{list.title}</h3>
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