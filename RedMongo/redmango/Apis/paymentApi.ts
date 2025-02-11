import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const paymentApi = createApi({
    reducerPath: "paymentApi",
    baseQuery: fetchBaseQuery({
        baseUrl: "http://localhost:5185/api",
    }),
    endpoints: (builder) => ({
        initialPayment: builder.mutation({
            query: (userData) => ({
                url: "/payments/create",
                method: "POST",
                body: userData,
                headers: {
                    "Content-Type": "application/json",
                },
            }),
        }),

    }),
});

export const { 
    useInitialPaymentMutation
} = paymentApi;

export default paymentApi;
