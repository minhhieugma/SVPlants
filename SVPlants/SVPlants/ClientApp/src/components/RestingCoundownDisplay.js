import React from 'react';

export default function RestingCoundownDisplay(props) {
  return (
    <div className="progress">
      <div className="progress-bar progress-bar-striped bg-warning progress-bar-animated" role="progressbar"
        aria-valuenow="75" aria-valuemin="0" aria-valuemax="100" style={{ width: `${props.remaining}%` }}></div>
    </div>
  )
}
