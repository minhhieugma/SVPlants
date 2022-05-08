import React from 'react';

export default function PlantStatusDisplay(props) {

  return (
    <button type="button" className={`btn btn-${props.btnStyle}`}
      disabled={props.processing}
      onClick={e => { props.onClick(); e.stopPropagation(); }}>
      {
        !props.processing ? props.display :
          <><span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
            {props.processingText}</>
      }
    </button>
  )
}
