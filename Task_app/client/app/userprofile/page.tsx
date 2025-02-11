"use client";

import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  signInWithEmailAndPassword,
  createUserWithEmailAndPassword,
  signOut,
  onAuthStateChanged,
  User,
} from "firebase/auth";
import { auth } from "@/firebase";

interface Profile {
  name: string;
  preferences: string;
}

interface UpdateData {
  name: string;
  preferences: string;
}

export default function UserProfile() {
  const [user, setUser] = useState<User | null>(null);
  const [profile, setProfile] = useState<Profile | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [updateData, setUpdateData] = useState<UpdateData>({ name: "", preferences: "" });
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  const handleLogin = async (): Promise<void> => {
    setError(null);
    setSuccessMessage(null);
    try {
      const result = await signInWithEmailAndPassword(auth, email, password);
      setUser(result.user);
      const token = await result.user.getIdToken();
      localStorage.setItem("token", token);
      fetchUserProfile(token, result.user.uid);
    } catch (err: any) {
      console.error("Login Error:", err);
      setError(err.message || "Failed to log in. Please try again.");
    }
  };

  const handleLogout = async (): Promise<void> => {
    setError(null);
    setSuccessMessage(null);
    try {
      await signOut(auth);
      setUser(null);
      setProfile(null);
      localStorage.removeItem("token");
    } catch (err: any) {
      console.error("Logout Error:", err);
      setError("Failed to log out. Please try again.");
    }
  };

  const fetchUserProfile = async (token: string, userId: string): Promise<void> => {
    setLoading(true);
    setError(null);
    try {
      const response = await axios.get<Profile>(`http://localhost:5003/api/users/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProfile(response.data);
      setUpdateData({
        name: response.data.name,
        preferences: response.data.preferences,
      });
    } catch (err: any) {
      console.error("Error fetching profile:", err);
      setError("Failed to fetch user profile.");
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateProfile = async (): Promise<void> => {
    if (!user) {
      setError("User is not logged in.");
      return;
    }
    const token = localStorage.getItem("token");
    if (!token) {
      setError("Authentication token is missing.");
      return;
    }
    setError(null);
    setSuccessMessage(null);
    try {
      await axios.put(`http://localhost:5003/api/users/${user.uid}`, updateData, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setSuccessMessage("Profile updated successfully!");
      fetchUserProfile(token, user.uid);
    } catch (err: any) {
      console.error("Error updating profile:", err);
      setError("Failed to update profile. Please try again.");
    }
  };

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (currentUser) => {
      if (currentUser) {
        setUser(currentUser);
        const token = await currentUser.getIdToken();
        localStorage.setItem("token", token);
        fetchUserProfile(token, currentUser.uid);
      }
    });
    return () => unsubscribe();
  }, []);

  if (!user) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
        <div className="max-w-md w-full bg-white p-8 rounded-lg shadow-md">
          <h2 className="text-2xl font-bold mb-6 text-center">Login</h2>
          {error && (
            <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
              {error}
            </div>
          )}
          <div className="mb-4">
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-3 py-2 border rounded focus:outline-none focus:ring focus:border-blue-300"
            />
          </div>
          <div className="mb-6">
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full px-3 py-2 border rounded focus:outline-none focus:ring focus:border-blue-300"
            />
          </div>
          <div className="flex justify-between">
            <button
              onClick={handleLogin}
              className="w-1/2 bg-blue-500 hover:bg-blue-600 text-white py-2 rounded mr-2 transition"
            >
              Login
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-100 py-8 px-4">
      <div className="max-w-xl mx-auto bg-white p-8 rounded-lg shadow-md">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold">Welcome, {user.email}</h2>
          <button
            onClick={handleLogout}
            className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded transition"
          >
            Logout
          </button>
        </div>
        {loading ? (
          <p className="text-center">Loading...</p>
        ) : (
          <div>
            {error && (
              <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
                {error}
              </div>
            )}
            {successMessage && (
              <div className="mb-4 p-3 bg-green-100 border border-green-400 text-green-700 rounded">
                {successMessage}
              </div>
            )}
            <div className="mb-4">
              <p className="font-medium">Name:</p>
              <p>{profile?.name || "N/A"}</p>
            </div>
            <div className="mb-6">
              <p className="font-medium">Preferences:</p>
              <p>{profile?.preferences || "N/A"}</p>
            </div>
            <div className="mb-6">
              <h3 className="text-xl font-semibold mb-4">Update Profile</h3>
              <div className="mb-4">
                <input
                  type="text"
                  placeholder="Update Name"
                  value={updateData.name}
                  onChange={(e) =>
                    setUpdateData({ ...updateData, name: e.target.value })
                  }
                  className="w-full px-3 py-2 border rounded focus:outline-none focus:ring focus:border-blue-300"
                />
              </div>
              <div className="mb-4">
                <input
                  type="text"
                  placeholder="Update Preferences"
                  value={updateData.preferences}
                  onChange={(e) =>
                    setUpdateData({ ...updateData, preferences: e.target.value })
                  }
                  className="w-full px-3 py-2 border rounded focus:outline-none focus:ring focus:border-blue-300"
                />
              </div>
              <button
                onClick={handleUpdateProfile}
                className="w-full bg-blue-500 hover:bg-blue-600 text-white py-2 rounded transition"
              >
                Update Profile
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
