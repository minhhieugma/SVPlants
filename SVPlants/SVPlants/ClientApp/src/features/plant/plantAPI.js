
export function fetchPlants() {
  return fetch("plant")
    .then(res => res.json())
}

export function startWatering(plantIds) {
  const requestOptions = {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ ids: plantIds })
  };

  return fetch(`plant/water/start`, requestOptions)
    .then(async response => {
      const isJson = response.headers.get('content-type')?.includes('application/json');
      const data = isJson ? await response.json() : null;

      if (!response.ok) {
        const error = (data && data.message) || (data && data.error) || response.status;
        return Promise.reject(error);
      }

      return data
    })
}

export function stopWatering(plantId) {
  const requestOptions = {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: ''
  };

  return fetch(`plant/${plantId}/water/stop`, requestOptions)
    .then(async response => {
      const isJson = response.headers.get('content-type')?.includes('application/json');
      const data = isJson ? await response.json() : null;

      if (!response.ok) {
        const error = (data && data.message) || (data && data.error) || response.status;
        return Promise.reject(error);
      }

      return data
    })
}
