import { HubConnectionBuilder } from "@microsoft/signalr";

let connection = <any>null;

export const startSignalRConnection = async (userId: number, onStatusUpdate: any) => {
    connection = new HubConnectionBuilder()
        .withUrl("/order-status-hub")
        .withAutomaticReconnect()
        .build();

    connection.on("OrderStatusUpdated", (data: any) => {
        console.log("Order status updated:", data);
        if (onStatusUpdate) {
            onStatusUpdate(data);
        }
    });

    try {
        await connection.start();
        console.log("SignalR connected");
        await connection.invoke("JoinGroup", userId);
    } catch (error) {
        console.error("SignalR connection error:", error);
    }
};

export const stopSignalRConnection = async (userId: number) => {
    if (connection) {
        await connection.invoke("LeaveGroup", userId);
        await connection.stop();
        console.log("SignalR disconnected");
    }
};
