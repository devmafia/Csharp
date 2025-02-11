"use client";

import { useEffect, useState } from "react";
import { useAuthGuard } from "@/authGuard";
import { auth } from "@/firebase";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

interface Task {
  id: number;
  title: string;
  description: string;
}

interface NewTask {
  title: string;
  description: string;
}

export default function TaskManager() {
  const user = useAuthGuard();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [newTask, setNewTask] = useState<NewTask>({ title: "", description: "" });
  const [editingTask, setEditingTask] = useState<Task | null>(null);

  useEffect(() => {
    if (!user) return;

    const fetchTasks = async () => {
      try {
        const token = await auth.currentUser?.getIdToken();
        console.log(token);
        const res = await fetch("http://localhost:5001/api/tasks", {
          headers: { Authorization: `Bearer ${token}` },
        });
        const data = await res.json();
        console.log("Fetched tasks:", data);
        if (Array.isArray(data)) {
          setTasks(data);
        } else if (data && Array.isArray(data.tasks)) {
          setTasks(data.tasks);
        } else {
          console.error("Unexpected data format:", data);
          setTasks([]);
        }
      } catch (error) {
        console.error("Failed to fetch tasks", error);
      } finally {
        setLoading(false);
      }
    };

    fetchTasks();

    const connection: HubConnection = new HubConnectionBuilder()
      .withUrl("http://localhost:5002/notifications")
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .catch((err: Error) => console.error("SignalR Connection Error", err));

    connection.on("TaskUpdated", (notification: any) => {
      console.log("Received notification:", notification);
      fetchTasks();
    });

    connection.on("TaskCreated", (createdTask: Task) => {
      setTasks((prevTasks) => [...prevTasks, createdTask]);
    });
    connection.on("TaskDeleted", (deletedTaskId: number) => {
      setTasks((prevTasks) =>
        prevTasks.filter((task) => task.id !== deletedTaskId)
      );
    });

    return () => {
      connection.stop().catch((err: Error) => {
        if (err.message.includes("AbortError")) {
          console.warn("Connection stopped before handshake completed.");
        } else {
          console.error("Error stopping SignalR connection", err);
        }
      });
    };
  }, [user]);

  const handleCreateTask = async () => {
    try {
      const token = await auth.currentUser?.getIdToken();
      await fetch("http://localhost:5001/api/tasks", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newTask),
      });
      setNewTask({ title: "", description: "" });
    } catch (error) {
      console.error("Failed to create task", error);
    }
  };

  const handleUpdateTask = async () => {
    if (!editingTask) return;
    try {
      const token = await auth.currentUser?.getIdToken();
      await fetch(`http://localhost:5001/api/tasks/${editingTask.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(editingTask),
      });
      setEditingTask(null);
    } catch (error) {
      console.error("Failed to update task", error);
    }
  };

  const handleDeleteTask = async (taskId: number) => {
    try {
      const token = await auth.currentUser?.getIdToken();
      await fetch(`http://localhost:5001/api/tasks/${taskId}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });
    } catch (error) {
      console.error("Failed to delete task", error);
    }
  };

  if (!user) return <p className="text-center mt-8">Loading...</p>;

  return (
    <div className="container mx-auto px-4 py-8">
      <h2 className="text-3xl font-bold mb-6 text-center">Manage Tasks</h2>

      <div className="bg-white rounded-lg shadow p-6 mb-8">
        <h3 className="text-xl font-semibold mb-4">Add New Task</h3>
        <div className="flex flex-col sm:flex-row sm:items-center sm:space-x-4">
          <input
            type="text"
            placeholder="Task Title"
            value={newTask.title}
            onChange={(e) =>
              setNewTask({ ...newTask, title: e.target.value })
            }
            className="mb-4 sm:mb-0 p-3 border rounded w-full"
          />
          <input
            type="text"
            placeholder="Task Description"
            value={newTask.description}
            onChange={(e) =>
              setNewTask({ ...newTask, description: e.target.value })
            }
            className="mb-4 sm:mb-0 p-3 border rounded w-full"
          />
          <button
            onClick={handleCreateTask}
            className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-3 rounded transition"
          >
            Add Task
          </button>
        </div>
      </div>

      {editingTask && (
        <div className="bg-white rounded-lg shadow p-6 mb-8">
          <h3 className="text-xl font-semibold mb-4">Edit Task</h3>
          <div className="flex flex-col sm:flex-row sm:items-center sm:space-x-4">
            <input
              type="text"
              value={editingTask.title}
              onChange={(e) =>
                setEditingTask({ ...editingTask, title: e.target.value })
              }
              className="mb-4 sm:mb-0 p-3 border rounded w-full"
            />
            <input
              type="text"
              value={editingTask.description}
              onChange={(e) =>
                setEditingTask({
                  ...editingTask,
                  description: e.target.value,
                })
              }
              className="mb-4 sm:mb-0 p-3 border rounded w-full"
            />
            <button
              onClick={handleUpdateTask}
              className="bg-green-500 hover:bg-green-600 text-white px-6 py-3 rounded transition"
            >
              Update Task
            </button>
          </div>
        </div>
      )}

      {loading ? (
        <p className="text-center">Loading tasks...</p>
      ) : (
        <ul className="space-y-4">
          {tasks.map((task) => (
            <li
              key={task.id}
              className="bg-white rounded-lg shadow p-4 flex justify-between items-center"
            >
              <div>
                <h3 className="text-xl font-semibold">{task.title}</h3>
                <p className="text-gray-600">{task.description}</p>
              </div>
              <div className="flex space-x-2">
                <button
                  onClick={() => setEditingTask(task)}
                  className="bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2 rounded transition"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDeleteTask(task.id)}
                  className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded transition"
                >
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
