import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const authApi = createApi({
        reducerPath: "authApi",
        baseQuery: fetchBaseQuery({
            baseUrl: "http://localhost:5185/api"
        }),
        endpoints: (builder) => ({
            registerUser: builder.mutation({
                query: (userData) => ({
                    url: "auth/register",
                    method: "POST",
                    body: userData,
                    headers: {
                        "Content-type": "application/json"
                    }
                })
            }),
            loginUser: builder.mutation({
                query: (userCredentials) => ({
                    url: "auth/login",
                    method: "POST",
                    body: userCredentials,
                    headers: {
                        "Content-type": "application/json"
                    }
                })
            })
        })
})

export const {useLoginUserMutation, useRegisterUserMutation} = authApi;
export default authApi;
