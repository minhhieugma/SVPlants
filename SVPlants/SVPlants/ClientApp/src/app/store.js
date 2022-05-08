import { configureStore } from '@reduxjs/toolkit';
import plantReducer from '../features/plant/plantSlice';

export const store = configureStore({
  reducer: {
    plant: plantReducer,
  },
});
