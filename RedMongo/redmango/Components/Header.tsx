"use client";

import Image from "next/image";
import Link from "next/link";
import { useSelector, useDispatch } from "react-redux";
import userModel from "../Interfaces/userModel";
import { RootState } from "../Storage/Redux/store";
import { initialState, setLoggedInUser } from "../Storage/Redux/userAuthSlice";
import * as jwt_decode from 'jwt-decode';
import { useEffect } from "react";

export default function Header() {
    const userData: userModel = useSelector((state: RootState) => state.userAuthStore);
    const dispatch = useDispatch();
    useEffect(() => {
        const token = localStorage.getItem("token");
        if (token) {
            try {
                const { fullname, id, email, role }: userModel = jwt_decode.jwtDecode(token);
                dispatch(setLoggedInUser({ fullname, id, email, role }));
            } catch (error) {
                console.error("Invalid token:", error);
            }
        }
    }, [dispatch]);

    const handleLogout = () => {
        localStorage.removeItem("token");
        dispatch(setLoggedInUser({...initialState}));
        window.location.href = '/';
    }

    return (
        <div className="flex text-white bg-black justify-between items-center p-4">
            <div className="flex gap-4 items-center">
                <Image src="/Assets/Images/mango.png" width={50} height={50} alt="mango logo" />
                <ul className="flex gap-2 m-0 p-0 list-none">
                    <li><Link href="/">Home</Link></li>
                    <li><Link href="/orders">Orders</Link></li>
                    <li><Link href="/shoppingCart">Cart</Link></li>
                </ul>
            </div>
            <div className="flex gap-4 justify-center items-center">
                <ul className="flex gap-2 m-0 p-0 list-none">
                    {!userData.id && (
                        <>
                            <li className="w-24 h-10 bg-green-500 text-white flex justify-center items-center rounded-full">
                                <Link href="/register" className="flex justify-center items-center">Register</Link>
                            </li>
                            <li className="flex justify-center items-center">
                                <Link href="/login">Login</Link>
                            </li>
                        </>
                    )}
                    {userData.id && (
                        <>
                            <li className="p-2 w-full h-10 bg-green-500 text-white flex justify-center items-center rounded-full">
                                Welcome, {userData.fullname}
                            </li>
                            <li className="flex justify-center items-center">
                                <button onClick={handleLogout}>Logout</button>
                            </li>
                        </>
                    )}
                </ul>
            </div>
        </div>
    );
}
