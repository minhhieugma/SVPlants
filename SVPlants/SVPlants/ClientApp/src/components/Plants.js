import React, { useState, useEffect } from 'react';
import moment from 'moment';

export default function Plants() {
  const [error, setError] = useState(null);
  const [isLoaded, setIsLoaded] = useState(false);
  const [items, setItems] = useState([]);

  useEffect(() => {
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
        }
      )
  }, [])

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
              <td>{p.lastWateredAt ? moment(p.lastWateredAt).format("YYYY-MM-DD kk:mm:ss") : '-'}</td>
              <td>{p.status}</td>
              <td>
                <button type="button" className="btn btn-primary btn-sm">Water</button>
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
