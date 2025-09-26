// This component receives a list of boards as "props"
function BoardList({ boards }) {
    return (
      <div className="boards-container">
        {boards.map(board => (
          <div key={board.id} className="board">
            <h2>{board.title}</h2>
          </div>
        ))}
      </div>
    );
  }
  
  export default BoardList;