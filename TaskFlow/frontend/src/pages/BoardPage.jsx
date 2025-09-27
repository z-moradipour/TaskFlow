import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import '../App.css';

function BoardPage() {
  // The useParams hook reads the `boardId` from the URL (e.g., /board/1)
  const { boardId } = useParams(); 

  const [board, setBoard] = useState(null);
  const [lists, setLists] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // This useEffect will run once when the component loads
  useEffect(() => {
    const fetchData = async () => {
      try {
        // Fetch the specific board's details
        const boardResponse = await axios.get(`https://localhost:7289/api/Boards/${boardId}`);
        setBoard(boardResponse.data);

        // Fetch the lists associated with this board
        const listsResponse = await axios.get(`https://localhost:7289/api/Lists/board/${boardId}`);
        setLists(listsResponse.data);

      } catch (err) {
        setError('Failed to fetch board data.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [boardId]); // The effect depends on boardId

  // Show a loading message while data is being fetched
  if (loading) {
    return <div>Loading...</div>;
  }

  // Show an error message if the API call fails
  if (error) {
    return <div>{error}</div>;
  }

  return (
    <div className="board-page-container">
      <Link to="/" className="back-link">← Back to Boards</Link>
      <h1>{board?.title}</h1>

      <div className="lists-container">
        {lists.map(list => (
          <div key={list.id} className="list">
            <h3>{list.title}</h3>
            {/* Will add cards here later */}
          </div>
        ))}
        {/* Will add a "Create New List" form here later */}
      </div>
    </div>
  );
}

export default BoardPage;