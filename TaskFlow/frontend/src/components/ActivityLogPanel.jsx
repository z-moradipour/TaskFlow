import React from 'react';

function ActivityLogPanel({ activities, isOpen, onClose }) {
  if (!isOpen) {
    return null;
  }

  return (
    <>
      {/* This overlay will dim the background when the panel is open */}
      <div className="activity-log-overlay" onClick={onClose}></div>
      
      <div className="activity-log-panel">
        <div className="activity-log-header">
          <h3>Activity</h3>
          <button onClick={onClose} className="close-button">×</button>
        </div>
        <ul className="activity-list">
          {activities.length > 0 ? (
            activities.map(log => (
              <li key={log.id}>
                <p className="activity-description">
                  <strong>{log.username}</strong> {log.description.replace(log.username, '')}
                </p>
                <span className="activity-timestamp">
                  {new Date(log.timestamp).toLocaleString()}
                </span>
              </li>
            ))
          ) : (
            <p>No recent activity.</p>
          )}
        </ul>
      </div>
    </>
  );
}

export default ActivityLogPanel;