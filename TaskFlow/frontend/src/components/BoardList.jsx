// src/components/BoardList.jsx
import { Link } from 'react-router-dom';

function BoardList({ boards }) {
  return (
    <div className="boards-container">
      {boards.map(board => (
        <Link key={board.id} to={`/board/${board.id}`} className="board-link">
          <div className="board">
            <h2>{board.title}</h2>
          </div>
        </Link>
      ))}
    </div>
  );
}

export default BoardList;