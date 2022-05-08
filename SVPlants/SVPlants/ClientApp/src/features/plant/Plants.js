import React, { useState, useEffect } from 'react';
import Swal from 'sweetalert2'
import PlantStatusDisplay from './shared/PlantStatusDisplay'
import Button from '../../components/Button'
import RestingCoundownDisplay from './shared/RestingCoundownDisplay'
import DateTimeDisplay from '../../components/DateTimeDisplay'

import { useSelector, useDispatch } from 'react-redux';
import {
  fetchPlantsAsync,
  startWateringAsync,
  stopWateringAsync,
  calculatePlantsStatus,
  selectPlants,
  selectStatus
} from './plantSlice';

export default function Plants() {
  const dispatch = useDispatch();

  const plants = useSelector(selectPlants);
  const status = useSelector(selectStatus);

  useEffect(() => {
    dispatch(fetchPlantsAsync())
  }, [])

  useEffect(() => {
    const interval = setInterval(() => {
      dispatch(calculatePlantsStatus())
    }, 1000);

    return () => clearInterval(interval);
  }, [])

  const renderActions = (plant) => {
    if (plant.isWatering)
      return <Button btnStyle='danger' display='Stop Watering' processing={plant.processing} processingText='Stopping...'
        onClick={() => dispatch(stopWateringAsync(plant.id))} />

    if (plant.status === 'Resting')
      return <RestingCoundownDisplay remaining={plant.remaining} />

    if (plant.status === 'Normal' || plant.status === 'NeededWater')
      return <Button btnStyle='primary' display='Start Watering' processing={plant.processing} processingText='Watering...'
        onClick={() => dispatch(startWateringAsync(plant.id))} />

  }

  return (
    status !== 'idle' ? 'processing' : <div>
      <table className="table">
        <thead>
          <tr>
            <th scope="col">#</th>
            <th scope="col">Plant Name</th>
            <th scope="col">Location</th>
            <th scope="col">Last Watered At</th>
            <th scope="col">Status</th>
            <th scope="col">Action</th>
          </tr>
        </thead>
        <tbody>
          {
            plants.map((p, index) =>
              <tr key={p.id}>
                <th scope="row">{index + 1}</th>
                <td>{p.name}</td>
                <td>{p.location}</td>
                <td><DateTimeDisplay value={p.lastWateredAt} /></td>
                <td><PlantStatusDisplay status={p.status} /></td>
                <td>{renderActions(p)}
                </td>
              </tr>
            )}
        </tbody>
      </table>
    </div>

  );
}
