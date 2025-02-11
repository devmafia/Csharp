"use client";

import { useEffect, useState } from "react";

const withAuth = (WrappedComponent: any) => {
    return (props: any) => {
        const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
        const isBrowser = typeof window !== "undefined";
        useEffect(() => {
            if (!isBrowser) {
                console.log("Not running in a browser environment.");
                return;
            }

            const accessToken = localStorage.getItem("token");
            if (!accessToken) {
                window.location.href = "/";
            } else {
                setIsAuthenticated(true);
            }
        }, []);

        if (isAuthenticated === null) {
            return <div>Loading...</div>;
        }

        return <WrappedComponent {...props} />;
    };
};

export default withAuth;
