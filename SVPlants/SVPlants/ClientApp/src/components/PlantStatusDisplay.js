import React from 'react';

export default function PlantStatusDisplay(props) {
  let type = ''

  switch (props.status) {
    case 'NeededWater':
      type = 'bg-danger'
      break
    case 'Watering':
      type = 'bg-primary'
      break
    case 'Resting':
      type = 'bg-warning text-dark'
      break
    default:
      type = 'bg-secondary'
      break
  }

  return (
    <span className={`badge rounded-pill ${type}`}>{props.status}</span>
  )
}
