import React, { useState, useEffect } from 'react';
import moment from 'moment';
import Swal from 'sweetalert2'
import PlantStatusDisplay from './PlantStatusDisplay'
import Button from './Button'
import RestingCoundownDisplay from './RestingCoundownDisplay'
import DateTimeDisplay from './DateTimeDisplay'

export default function Plants() {
  const [error, setError] = useState(null);
  const [isLoaded, setIsLoaded] = useState(false);
  const [items, setItems] = useState([]);

  function reloadData() {
    // setIsLoaded(false);
    fetch("plant")
      .then(res => res.json())
      .then(
        (result) => {

          setIsLoaded(true);
          setItems(result);
        },
        // Note: it's important to handle errors here
        // instead of a catch() block so that we don't swallow
        // exceptions from actual bugs in components.
        (error) => {
          setIsLoaded(true);
          setError(error);

          Swal.fire({
            title: 'Error!',
            text: 'Do you want to continue',
            icon: 'error',
            confirmButtonText: 'Cool'
          })
        }
      )
  }

  useEffect(() => {
    reloadData()
  }, [])

  useEffect(() => {
    const interval = setInterval((items) => {
      // reloadData()

      items.forEach(plant => {
        const duration = moment.duration(moment().diff(moment(moment(plant.lastWateredAt))))

        if (duration.asHours() >= 6) {
          setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...el, status: 'NeededWater' } : el))))
        }
        else if (plant.status === 'Resting') {
          let remaining = (1 - duration.asSeconds() / 30) * 100
          let status = plant.status

          if (remaining <= 0) {
            status = 'Normal'
            remaining = 0
          }

          setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...el, status: status, remaining: remaining } : el))))
        }

      })

    }, 1000, items);

    return () => clearInterval(interval);
  }, [items])

  useEffect(() => {
    if (!error) return

    Swal.fire({
      title: 'Error!',
      text: error.message,
      icon: 'error',
      confirmButtonText: 'Ok'
    })
  }, [error])

  function startWater(plant) {
    setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...el, processing: true } : el))))

    const requestOptions = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: ''
    };

    fetch(`plant/${plant.id}/water/start`, requestOptions)
      .then(async response => {
        const isJson = response.headers.get('content-type')?.includes('application/json');
        const data = isJson ? await response.json() : null;

        if (!response.ok) {
          const error = (data && data.message) || response.status;
          return Promise.reject(error);
        }

        return data
      })
      .then(
        (result) => {
          setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...result, processing: false } : el))))
        },
        (error) => {
          setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...el, processing: false } : el))))
          setError(error);
        }
      )
  }

  function stopWater(plant) {
    setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...el, processing: true } : el))))

    const requestOptions = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: ''
    };

    fetch(`plant/${plant.id}/water/stop`, requestOptions)
      .then(async response => {
        const isJson = response.headers.get('content-type')?.includes('application/json');
        const data = isJson ? await response.json() : null;

        if (!response.ok) {
          const error = (data && data.message) || response.status;
          return Promise.reject(error);
        }

        return data
      })
      .then(
        (result) => {
          setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...result, processing: false } : el))))
        },
        (error) => {
          setItems(prevState => (prevState.map(el => (el.id === plant.id ? { ...el, processing: false } : el))))
          setError(error);
        }
      )
  }

  const renderActions = (plant) => {
    if (plant.status === 'Watering')
      return <Button btnStyle='danger' display='Stop Watering' processing={plant.processing} onClick={() => stopWater(plant)} />

    if (plant.status === 'Resting')
      return <RestingCoundownDisplay remaining={plant.remaining} />

    if (plant.status === 'Normal' || plant.status === 'NeededWater')
      return <Button btnStyle='primary' display='Start Watering' processing={plant.processing} processingText='Watering...' onClick={() => startWater(plant)} />

  }
  
  return (

    <div>
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
          {items.map((p, index) =>
            <tr key={p.id}>
              <th scope="row">{index + 1}</th>
              <td>{p.name}</td>
              <td>{p.location}</td>
              <td><DateTimeDisplay value={p.lastWateredAt} /></td>
              <td><PlantStatusDisplay status={p.status} /></td>
              <td>
                {
                  renderActions(p)
                }
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
