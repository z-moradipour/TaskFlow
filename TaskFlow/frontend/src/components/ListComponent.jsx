import { useDroppable } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { DraggableCard } from './DraggableCard.jsx';

// Note: We are temporarily removing the "Add Card" form to isolate the DnD logic.
function ListComponent({ list, onDeleteList }) {
  const { setNodeRef } = useDroppable({ id: `list-${list.id}` });

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
            <DraggableCard key={card.id} card={card} />
          ))}
        </SortableContext>
      </div>
    </div>
  );
}

export default ListComponent;