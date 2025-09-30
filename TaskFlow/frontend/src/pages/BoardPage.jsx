import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import { DndContext, closestCenter, PointerSensor, useSensor, useSensors } from '@dnd-kit/core';
import { arrayMove } from '@dnd-kit/sortable';
import ListComponent from '../components/ListComponent.jsx';
import '../App.css';

function BoardPage() {
    const { boardId } = useParams();
    const [lists, setLists] = useState([]);
    const [loading, setLoading] = useState(true);

    const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 10 } }));

    useEffect(() => {
        // ... (Your data fetching useEffect remains the same) ...
        const fetchData = async () => {
            try {
                const boardResponse = await axios.get(`https://localhost:7289/api/Boards/${boardId}`);
                setLists(boardResponse.data.lists || []);
            } catch (err) { console.error(err); } finally { setLoading(false); }
        };
        fetchData();
    }, [boardId]);
    
    function handleDragEnd(event) {
        const { active, over } = event;
        if (!over || active.id === over.id) return;
    
        setLists((prevLists) => {
            const sourceList = prevLists.find(list => list.cards.some(c => `card-${c.id}` === active.id));
            const destList = prevLists.find(list => `list-${list.id}` === over.id || list.cards.some(c => `card-${c.id}` === over.id));
            if (!sourceList || !destList) return prevLists;
    
            const activeCardId = Number(String(active.id).replace('card-', ''));
            const [movedCard] = sourceList.cards.filter(c => c.id === activeCardId);
    
            let newLists = JSON.parse(JSON.stringify(prevLists));
            let sourceListInNewState = newLists.find(l => l.id === sourceList.id);
            let destListInNewState = newLists.find(l => l.id === destList.id);
            
            const sourceCardIndex = sourceListInNewState.cards.findIndex(c => c.id === activeCardId);
            sourceListInNewState.cards.splice(sourceCardIndex, 1);
            
            const overIsListContainer = over.id.toString().startsWith('list-');
            const overCardIndex = overIsListContainer ? destListInNewState.cards.length : destListInNewState.cards.findIndex(c => `card-${c.id}` === over.id);
            destListInNewState.cards.splice(overCardIndex, 0, movedCard);
    
            if (sourceList.id === destList.id) {
                const orderedCardIds = destListInNewState.cards.map(c => c.id);
                axios.put(`https://localhost:7289/api/Cards/reorder/${destList.id}`, { orderedCardIds });
            } else {
                axios.put(`https://localhost:7289/api/Cards/reorder/${sourceList.id}`, { orderedCardIds: sourceListInNewState.cards.map(c => c.id) });
                axios.put(`https://localhost:7289/api/Cards/reorder/${destList.id}`, { orderedCardIds: destListInNewState.cards.map(c => c.id) });
            }
            
            return newLists;
        });
    }
      
    if (loading) return <div>Loading...</div>;

    return (
        <DndContext sensors={sensors} onDragEnd={handleDragEnd} collisionDetection={closestCenter}>
            <div className="board-page-container">
                <Link to="/" className="back-link">← Back to Boards</Link>
                <h1>Board Title</h1>
                <div className="lists-container">
                    {lists.map(list => (
                        <ListComponent
                            key={list.id}
                            list={list}
                            onDeleteList={() => {}} // Delete temporarily disabled
                        />
                    ))}
                </div>
            </div>
        </DndContext>
    );
}

export default BoardPage;