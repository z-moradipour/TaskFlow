import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import { DndContext, closestCorners } from '@dnd-kit/core';
import ListComponent from '../components/ListComponent.jsx';
import '../App.css';

function BoardPage() {
  const { boardId } = useParams();
  const [board, setBoard] = useState(null);
  const [lists, setLists] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [newListTitle, setNewListTitle] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const boardResponse = await axios.get(`https://localhost:7289/api/Boards/${boardId}`);
        setBoard(boardResponse.data);
        // The lists are now part of the boardDto, so we don't need a separate call
        setLists(boardResponse.data.lists || []);
      } catch (err) {
        setError('Failed to fetch board data.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [boardId]);

  const handleCreateList = async (e) => {
    e.preventDefault();
    if (!newListTitle.trim()) return;
    try {
      const newListDto = { title: newListTitle };
      const response = await axios.post(`https://localhost:7289/api/Lists/board/${boardId}`, newListDto);
      setLists([...lists, { ...response.data, cards: [] }]);
      setNewListTitle('');
    } catch (err) {
      console.error("Error creating list:", err);
    }
  };

  const handleDeleteList = async (listId) => {
    try {
      await axios.delete(`https://localhost:7289/api/Lists/${listId}`);
      setLists(lists.filter(list => list.id !== listId));
    } catch (err) {
      console.error("Error deleting list:", err);
    }
  };

  const handleDragEnd = async (event) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const activeCardId = Number(active.id.replace('card-', ''));
    const destListId = Number(over.id.replace('list-', ''));

    let originalListId;
    let draggedCard;

    // Find the card and its original list
    for (const list of lists) {
        const card = list.cards.find(c => c.id === activeCardId);
        if (card) {
            draggedCard = card;
            originalListId = list.id;
            break;
        }
    }

    if (!draggedCard || originalListId === destListId) return;

    // Optimistic UI Update
    setLists(prevLists => {
        const newLists = prevLists.map(list => ({
            ...list,
            cards: [...list.cards]
        }));
        
        const sourceList = newLists.find(l => l.id === originalListId);
        const destList = newLists.find(l => l.id === destListId);
        
        const cardIndex = sourceList.cards.findIndex(c => c.id === activeCardId);
        sourceList.cards.splice(cardIndex, 1);
        destList.cards.push(draggedCard);

        return newLists;
    });

    // API Call to persist the change
    try {
        await axios.put(`https://localhost:7289/api/Cards/${activeCardId}/move`, destListId, {
            headers: { 'Content-Type': 'application/json' }
        });
    } catch (err) {
        console.error("Failed to move card:", err);
        // Here you could add logic to revert the UI change on API failure
    }
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;

  return (
    <DndContext onDragEnd={handleDragEnd} collisionDetection={closestCorners}>
      <div className="board-page-container">
        <Link to="/" className="back-link">← Back to Boards</Link>
        <h1>{board?.title}</h1>
        <div className="lists-container">
          {lists.map(list => (
            <ListComponent
              key={list.id}
              list={list}
              onDeleteList={handleDeleteList}
              setLists={setLists} // Pass setLists down for card creation
            />
          ))}
          <div className="list add-list-form">
            <form onSubmit={handleCreateList}>
              <input
                type="text"
                value={newListTitle}
                onChange={(e) => setNewListTitle(e.target.value)}
                placeholder="Enter list title..."
              />
              <button type="submit">Add List</button>
            </form>
          </div>
        </div>
      </div>
    </DndContext>
  );
}

export default BoardPage;