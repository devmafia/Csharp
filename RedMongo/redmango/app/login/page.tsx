"use client";
import { useState } from "react";
import styles from "../page.module.css";
import { useLoginUserMutation } from "../../Apis/authApi";
import * as jwt_decode from "jwt-decode";
import userModel from "../../Interfaces/userModel";
import { useDispatch } from "react-redux";
import { setLoggedInUser } from "../../Storage/Redux/userAuthSlice";
import { useRouter } from "next/navigation";

export default function Login() {
    const [loginUser] = useLoginUserMutation();
    const [formData, setFormData] = useState({
        email: "",
        password: ""
    });
    const dispatch = useDispatch();
    const router = useRouter();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { id, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [id]: value
        }))
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>)=> {
        e.preventDefault();

        const res = await loginUser(formData)

        if (res.data) {
            const token = res.data.result.token;
            const { fullname, id, email, role } : userModel = jwt_decode.jwtDecode(token);
            localStorage.setItem("token", token);
            dispatch(setLoggedInUser({ fullname, id, email, role }));
            router.push("/");
        }
    }


    return (
        <div className={styles.page}>
            <div className="container text-center">
            <form onSubmit={handleSubmit} className="max-w-md mx-auto bg-white p-6 rounded-lg shadow-md">
                <h1 className="text-2xl font-bold text-center text-gray-800 mb-6">Login</h1>
                <div className="mb-4">
                    <label
                    htmlFor="email"
                    className="block text-sm font-medium text-gray-700 mb-1"
                    >
                    Email
                    </label>
                    <input
                    onChange={handleInputChange}
                    id="email"
                    type="text"
                    placeholder="Enter email"
                    required
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
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
                    onChange={handleInputChange}
                    id="password"
                    type="password"
                    placeholder="Enter password"
                    required
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    />
                </div>

                <div className="text-center">
                    <button
                    type="submit"
                    className="w-full bg-green-600 text-white font-medium py-2 px-4 rounded-md shadow hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 transition"
                    >
                    Login
                    </button>
                </div>
            </form>
            </div>
        </div>
    );
}
