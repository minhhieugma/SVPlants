import React, { Component } from 'react';

export class Plants extends Component {

  constructor(props) {
    super(props);
    this.state = { currentCount: 0 };
    this.incrementCounter = this.incrementCounter.bind(this);
  }

  incrementCounter() {
    this.setState({
      currentCount: this.state.currentCount + 1
    });
  }

  render() {
    return (
      <div>
        <table class="table">
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
            <tr>
              <th scope="row">1</th>
              <td>American Marigold</td>
              <td>Front Door</td>
              <td>2020-05-05 14:40</td>
              <td>Normal</td>
              <td>
                <button type="button" class="btn btn-primary btn-sm">Water</button>
              </td>
            </tr>
            <tr>
              <th scope="row">1</th>
              <td>American Marigold</td>
              <td>Front Door</td>
              <td>2020-05-05 14:40</td>
              <td>Normal</td>
              <td>
                <button type="button" class="btn btn-primary btn-sm">Water</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    );
  }
}
