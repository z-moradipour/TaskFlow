import { useState, useEffect } from 'react';
import api from './api/axiosConfig.js';
import './App.css';
import BoardList from './components/BoardList.jsx';
import AddBoardForm from './components/AddBoardForm.jsx';

function App() {
  const [boards, setBoards] = useState([]);

  useEffect(() => {
    const fetchBoards = async () => {
      try {
        const response = await api.get('https://localhost:7289/api/Boards');
        setBoards(response.data);
      } catch (error) {
        console.error("There was an error fetching the boards:", error);
      }
    };
    fetchBoards();
  }, []);

  const handleCreateBoard = async (title) => {
    try {
      const newBoard = { title };
      const response = await api.post('https://localhost:7289/api/Boards', newBoard);
      setBoards([...boards, response.data]);
    } catch (error) {
      console.error("There was an error creating the board:", error);
    }
  };

  return (
    <div className="app-container">
      <h1>TaskFlow Boards</h1>

      {/* Pass the logic to 'form component' */}
      <AddBoardForm onCreate={handleCreateBoard} />

      {/* Pass the data to 'list component' */}
      <BoardList boards={boards} />
    </div>
  );
}

export default App;