import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import { DndContext, DragOverlay, closestCorners, PointerSensor, useSensor, useSensors } from '@dnd-kit/core';
import { arrayMove } from '@dnd-kit/sortable';
import ListComponent from '../components/ListComponent.jsx';
import CardModal from '../components/CardModal.jsx';
import { DraggableCard } from '../components/DraggableCard.jsx';
import '../App.css';

function BoardPage() {
    const { boardId } = useParams();
    const [board, setBoard] = useState(null);
    const [lists, setLists] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [newListTitle, setNewListTitle] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedCard, setSelectedCard] = useState(null);
    const [activeCard, setActiveCard] = useState(null);

    const sensors = useSensors(
        useSensor(PointerSensor, {
            activationConstraint: { distance: 10 },
        })
    );

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const boardResponse = await axios.get(`https://localhost:7289/api/Boards/${boardId}`);
                setBoard(boardResponse.data);
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
        if (window.confirm("Are you sure you want to delete this list and all its cards?")) {
            try {
                await axios.delete(`https://localhost:7289/api/Lists/${listId}`);
                setLists(lists.filter(list => list.id !== listId));
            } catch (err) {
                console.error("Error deleting list:", err);
            }
        }
    };

    const openCardModal = (card) => {
        setSelectedCard(card);
        setIsModalOpen(true);
    };

    const closeCardModal = () => {
        setIsModalOpen(false);
        setSelectedCard(null);
    };

    const handleSaveCard = async (updatedCard) => {
        try {
            const updateDto = {
                title: updatedCard.title,
                description: updatedCard.description,
            };
            await axios.put(`https://localhost:7289/api/Cards/${updatedCard.id}`, updateDto);
            setLists(prevLists => 
                prevLists.map(list => ({
                    ...list,
                    cards: list.cards.map(card => 
                        card.id === updatedCard.id ? { ...card, ...updateDto } : card
                    )
                }))
            );
            closeCardModal();
        } catch (error) {
            console.error("Failed to save card:", error);
        }
    };

    const handleDeleteCard = async (cardIdToDelete) => {
        try {
            await axios.delete(`https://localhost:7289/api/Cards/${cardIdToDelete}`);
            setLists(prevLists => 
                prevLists.map(list => ({
                    ...list,
                    cards: list.cards.filter(card => card.id !== cardIdToDelete)
                }))
            );
            closeCardModal();
        } catch (error) {
            console.error("Failed to delete card:", error);
        }
    };

    function handleDragStart(event) {
        const { active } = event;
        const cardId = Number(active.id.toString().replace('card-', ''));
        for (const list of lists) {
            const foundCard = list.cards.find(c => c.id === cardId);
            if (foundCard) {
                setActiveCard(foundCard);
                break;
            }
        }
    }

    function handleDragEnd(event) {
        const { active, over } = event;
        setActiveCard(null);
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
            destListInNewState.cards.splice(overCardIndex >= 0 ? overCardIndex : destListInNewState.cards.length, 0, movedCard);

            if (sourceList.id === destList.id) {
                const orderedCardIds = destListInNewState.cards.map(c => c.id);
                axios.put(`https://localhost:7289/api/Cards/reorder/${destList.id}`, { orderedCardIds });
            } else {
                const sourceCardIds = sourceListInNewState.cards.map(c => c.id);
                axios.put(`https://localhost:7289/api/Cards/reorder/${sourceList.id}`, { orderedCardIds: sourceCardIds });
                
                const destCardIds = destListInNewState.cards.map(c => c.id);
                axios.put(`https://localhost:7289/api/Cards/reorder/${destList.id}`, { orderedCardIds: destCardIds });
            }
            
            return newLists;
        });
    }

    if (loading) return <div>Loading...</div>;
    if (error) return <div>{error}</div>;

    return (
        <DndContext
            sensors={sensors}
            onDragStart={handleDragStart}
            onDragEnd={handleDragEnd}
            collisionDetection={closestCorners}
        >
            <div className="board-page-container">
                <Link to="/" className="back-link">← Back to Boards</Link>
                <h1>{board?.title}</h1>
                <div className="lists-container">
                    {lists.map(list => (
                        <ListComponent
                            key={list.id}
                            list={list}
                            onDeleteList={handleDeleteList}
                            setLists={setLists}
                            onCardClick={openCardModal}
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
            <CardModal 
                isOpen={isModalOpen}
                onRequestClose={closeCardModal}
                card={selectedCard}
                onSave={handleSaveCard}
                onDelete={handleDeleteCard}
            />
            <DragOverlay>
                {activeCard ? <DraggableCard card={activeCard} /> : null}
            </DragOverlay>
        </DndContext>
    );
}

export default BoardPage;