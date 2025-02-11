"use client";

import { Provider, useDispatch } from "react-redux";
import store from "./Redux/store";

export default function Providers({ children }: { children: React.ReactNode }) {
  return <Provider store={store}>{children}</Provider>;
}
