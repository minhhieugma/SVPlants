import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { fetchPlants, startWatering, stopWatering } from './plantAPI';
import moment from 'moment';

const initialState = {
  plants: [],
  status: 'idle'
};

export const fetchPlantsAsync = createAsyncThunk(
  'plant/fetchPlants',
  async () => {
    const response = await fetchPlants();
    return response;
  }
);

export const startWateringAsync = createAsyncThunk(
  'plant/startWatering',
  async (plantId) => {
    const response = await startWatering(plantId);
    return response;
  }
);

export const stopWateringAsync = createAsyncThunk(
  'plant/stopWatering',
  async (plantId) => {
    const response = await stopWatering(plantId);
    return response;
  }
);

export const plantSlice = createSlice({
  name: 'plant',
  initialState,
  reducers: {
    calculatePlantsStatus: (state) => {

      state.plants.forEach(plant => {
        const duration = moment.duration(moment().diff(moment(moment(plant.lastWateredAt))))

        if (plant.isWatering) {
          plant.status = 'Watering'
        }
        else if (duration.asHours() >= 6) {
          plant.status = 'NeededWater'
        }
        else if (plant.status === 'Resting') {
          let remaining = (1 - duration.asSeconds() / 30) * 100
          let status = plant.status

          if (remaining <= 0) {
            status = 'Normal'
            remaining = 0
          }

          plant.status = status
          plant.remaining = remaining
        }
      })
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPlantsAsync.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchPlantsAsync.fulfilled, (state, action) => {
        state.status = 'idle';
        state.plants = action.payload;
      })
      .addCase(startWateringAsync.pending, (state, action) => {
        state.plants.find(p => p.id === action.meta.arg).processing = true
      })
      .addCase(startWateringAsync.fulfilled, (state, action) => {
        action.payload.processing = false
        state.plants = state.plants.map(el => (el.id === action.payload.id ? action.payload : el))
      })
      .addCase(startWateringAsync.rejected, (state, action) => {
        state.plants.find(p => p.id === action.meta.arg).processing = false
        state.error = action.error.message
      })
      .addCase(stopWateringAsync.pending, (state, action) => {
        state.plants.find(p => p.id === action.meta.arg).processing = true
      })
      .addCase(stopWateringAsync.fulfilled, (state, action) => {
        action.payload.processing = false
        state.plants = state.plants.map(el => (el.id === action.payload.id ? action.payload : el))
      })
      .addCase(stopWateringAsync.rejected, (state, action) => {
        state.plants.find(p => p.id === action.meta.arg).processing = false
        state.error = action.error.message
      })
  },
});

export const selectPlants = (state) => state.plant.plants;
export const selectStatus = (state) => state.plant.status;

export const { calculatePlantsStatus } = plantSlice.actions;

export default plantSlice.reducer;
