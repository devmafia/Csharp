import { createSlice } from "@reduxjs/toolkit";
import userModel from '../../Interfaces/userModel';

export const initialState: userModel = {
    fullname: "",
    id: "",
    email: "",
    role: ""
}

const userAuthSlice = createSlice({
    name: "userAuth",
    initialState: initialState,
    reducers: {
        setLoggedInUser: (state, action) => {
            state.fullname = action.payload.fullname;
            state.id = action.payload.id;
            state.email = action.payload.email;
            state.role = action.payload.role;
        }
    }
})

export const { setLoggedInUser } = userAuthSlice.actions;
export const userAuthReducer = userAuthSlice.reducer;
