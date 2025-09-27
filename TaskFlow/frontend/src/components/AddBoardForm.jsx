import { useState } from 'react';

/* This component takes a "prop" called onCreate. This prop is a function 
that is passed here from the parent component (App). */
function AddBoardForm({ onCreate }) {
  const [title, setTitle] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!title.trim()) return;
    
    onCreate(title);
    setTitle('');
  };

  return (
    <form onSubmit={handleSubmit} className="add-board-form">
      <input
        type="text"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="New board title"
      />
      <button type="submit">Create Board</button>
    </form>
  );
}

export default AddBoardForm;