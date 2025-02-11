"use client";

import styles from "../page.module.css";
import { useRegisterUserMutation } from "../../Apis/authApi";
import { useState } from "react";

export default function Register() {
    const [registerUser] = useRegisterUserMutation();
    const [formData, setFormData] = useState({
        email: "",
        name: "",
        password: ""
    })
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { id, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [id]: value
        }))
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>)=> {
        e.preventDefault();

        const res = await registerUser(formData)

        if (res.data) {
            console.log(res.data);
        }
    }

    return (
        <div className={styles.page}>
            <div className="container text-center">
            <form onSubmit={handleSubmit} className="max-w-md mx-auto bg-white p-6 rounded-lg shadow-md">
                <h1 className="text-2xl font-bold text-center text-gray-800 mb-6">Register</h1>
                <div className="mb-4">
                    <label
                    htmlFor="name"
                    className="block text-sm font-medium text-gray-700 mb-1"
                    >
                    Username
                    </label>
                    <input
                    id="name"
                    type="text"
                    placeholder="Enter username"
                    required
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    onChange={handleInputChange}
                    />
                </div>
                <div className="mb-4">
                    <label
                    htmlFor="email"
                    className="block text-sm font-medium text-gray-700 mb-1"
                    >
                    Email
                    </label>
                    <input
                    id="email"
                    type="text"
                    placeholder="Enter email"
                    required
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    onChange={handleInputChange}
                    />
                </div>
                <div className="mb-6">
                    <label
                    htmlFor="password"
                    className="block text-sm font-medium text-gray-700 mb-1"
                    >
                    Password
                    </label>
                    <input
                    id="password"
                    type="password"
                    placeholder="Enter password"
                    required
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    onChange={handleInputChange}
                    />
                </div>

                <div className="text-center">
                    <button
                    type="submit"
                    className="w-full bg-green-600 text-white font-medium py-2 px-4 rounded-md shadow hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 transition"
                    >
                    Register
                    </button>
                </div>
            </form>
            </div>
        </div>
    );
}
