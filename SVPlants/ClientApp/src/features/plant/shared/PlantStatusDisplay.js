import React from 'react';

export default function PlantStatusDisplay(props) {
  let type = ''
  let status = props.status

  switch (status) {
    case 'NeededWater':
      type = 'bg-danger'
      status = 'Need Water'
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
    <span className={`badge rounded-pill p-2 px-2 ${type}`}>{status}</span>
  )
}
