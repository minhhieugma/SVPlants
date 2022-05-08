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

export default function PlantsGrid() {
  const [error, setError] = useState(null);
  const [selectedPlants, setSelectedPlants] = useState([]);

  const dispatch = useDispatch();

  const plants = useSelector(selectPlants);
  const status = useSelector(selectStatus);

  useEffect(() => {
    dispatch(fetchPlantsAsync())
      .unwrap()
      .catch(e => setError(e))
  }, [])

  useEffect(() => {
    const interval = setInterval(() => {
      dispatch(calculatePlantsStatus())
    }, 1000);

    return () => clearInterval(interval);
  }, [])

  useEffect(() => {
    if (!error) return

    Swal.fire({
      title: 'Error!',
      text: error.message,
      icon: 'error',
      confirmButtonText: 'Ok'
    })
  }, [error])

  const renderActions = (plant) => {
    if (plant.isWatering)
      return <Button btnStyle='danger btn-sm' display='Stop Watering' processing={plant.processing} processingText='Stopping...'
        onClick={() => dispatch(stopWateringAsync(plant.id)).unwrap()
          .catch(e => setError(e))} />

    if (plant.status === 'Resting')
      return <RestingCoundownDisplay remaining={plant.remaining} />

    if (plant.status === 'Normal' || plant.status === 'NeededWater')
      return <Button btnStyle='primary btn-sm' display='Start Watering' processing={plant.processing} processingText='Watering...'
        onClick={() => dispatch(startWateringAsync([plant.id])).unwrap()
          .catch(e => setError(e))} />

  }

  const onSelectionChanged = (plant) => {
    setSelectedPlants(prevState => prevState.find(p => p === plant.id) !== undefined ? prevState.filter(p => p !== plant.id) : [...prevState, plant.id])
  }

  return (
    status !== 'idle' ?
      <div className="spinner-border text-primary" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      :
      <div>
        <Button btnStyle='primary' display='Water selected plants'
          onClick={() => {
            dispatch(startWateringAsync(selectedPlants)).unwrap()
            .catch(e => setError(e))

            setSelectedPlants([])
          }} />
        <table className="table table-hover">
          <thead>
            <tr>
              <th scope="col"></th>
              <th scope="col">#</th>
              <th scope="col">Image</th>
              <th scope="col">Plant Name</th>
              <th scope="col">Location</th>
              <th scope="col">Last Watered At</th>
              <th scope="col">Status</th>
              <th scope="col">Action</th>
            </tr>
          </thead>
          <tbody>
            {
              plants.map((plant, index) =>
                <tr key={plant.id} onClick={() => onSelectionChanged(plant)}>
                  <td>
                    <div className="form-check">
                        <input className="form-check-input" type="checkbox"
                            checked={selectedPlants.find(p => p === plant.id) !== undefined} />
                    </div>
                  </td>
                  <th scope="row">{index + 1}</th>
                  <td style={{ width: '15%' }}>
                    <img src={plant.imageUrl}
                      className="rounded img-fluid"></img>
                  </td>
                  <td>{plant.name}</td>
                  <td>{plant.location}</td>
                  <td><DateTimeDisplay value={plant.lastWateredAt} /></td>
                  <td><PlantStatusDisplay status={plant.status} /></td>
                  <td>{renderActions(plant)}</td>
                </tr>
              )}
          </tbody>
        </table>
      </div>

  );
}
