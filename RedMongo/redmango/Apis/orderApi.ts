import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const orderApi = createApi({
        reducerPath: "orderApi",
        baseQuery: fetchBaseQuery({
            baseUrl: "http://localhost:5185/api"
        }),
        tagTypes: ["Orders"],
        endpoints: (builder) => ({
            initialPayment: builder.mutation({
                query: (orderDetails) => ({
                    url: "/order",
                    method: "POST",
                    body: orderDetails,
                    headers: {
                        "Content-type": "application/json"
                    }
                }),
                invalidatesTags: ["Orders"]
            }),    
        getAllOrders: builder.query({
            query: (userId) => ({
                url: "/Order",
                params: {
                    userId: userId,
                },
            }),
            providesTags: ["Orders"]
        }),

        getOrderDetails: builder.query({
            query: (id) => ({
                url: `/Order/${id}`,
            }),
            providesTags: ["Orders"]
        }),
        })
})

export const { useInitialPaymentMutation, useGetAllOrdersQuery, 
    useGetOrderDetailsQuery  } = orderApi;
export default orderApi;
