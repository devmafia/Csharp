import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import menuItemApi from "./menuItemApi";

const shoppingCartApi = createApi({
        reducerPath: "shoppingCartApi",
        baseQuery: fetchBaseQuery({
            baseUrl: "http://localhost:5185/api"
        }),
        tagTypes: ["ShoppingCarts"],
        endpoints: (builder) => ({
            getShoppingCart: builder.query({
                query: (id) => ({
                    url: `/shoppingCart`,
                    params: {
                        userId: id
                    }
                }),
                providesTags: ["ShoppingCarts"]
            }),
            updateShoppingCart: builder.mutation({
                query: ({menuItemId, updateQuantityBy, userId}) => ({
                    url: "/shoppingCart",
                    method: "POST",
                    params: {
                        menuItemId,
                        updateQuantityBy,
                        userId
                    },
                    headers: {
                        "Content-type": "application/json"
                    }
                })
            })
        })
})

export const {useGetShoppingCartQuery, useUpdateShoppingCartMutation} = shoppingCartApi;
export default shoppingCartApi;
