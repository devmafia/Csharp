"use client";

import * as jwt_decode from "jwt-decode";

const withAdminAuth  = (WrappedComponent: any) => {
    return (props: any)=> {
        const accessToken = localStorage.getItem("token");
        if (!accessToken) {
            window.location.href = "/";
        }
        if (accessToken) {
            const decode: {
                role: string;
            } = jwt_decode.jwtDecode(accessToken);

            if(decode.role !== "Admin") {
                window.location.href = "/accessDenied";
            }
        } else {
            window.location.href = "/login";
        }

        return <WrappedComponent {...props}></WrappedComponent>
    }
}


export default withAdminAuth;
