import React, { useEffect, useState } from "react";
import { startSignalRConnection, stopSignalRConnection } from "./../Services/signalRService";

const OrderStatusTracker = ({ userId }: any) => {
    const [orderStatus, setOrderStatus] = useState("Pending");

    useEffect(() => {
        startSignalRConnection(userId, (data: any) => {
            setOrderStatus(data.status);
        });

        return () => {
            stopSignalRConnection(userId);
        };
    }, [userId]);

    return (
        <div className="mt-4 p-4 border-t border-gray-300">
            <h3 className="text-lg font-bold text-gray-800">Order Status</h3>
            <p className="text-gray-600">
                Current Status: <span className="font-semibold">{orderStatus}</span>
            </p>
        </div>
    );
};

export default OrderStatusTracker;
