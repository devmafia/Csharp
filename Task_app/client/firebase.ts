import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";

const firebaseConfig = {
  apiKey: "AIzaSyBRmjinnT3GMRh0K9tj7mALwvRH4tWW0gI",
  authDomain: "taskapp-e90d2.firebaseapp.com",
  projectId: "taskapp-e90d2",
  storageBucket: "taskapp-e90d2.firebasestorage.app",
  messagingSenderId: "462620734896",
  appId: "1:462620734896:web:16b9f80654005b3836d275",
  measurementId: "G-DG5HEDCZ62"
};

const app = initializeApp(firebaseConfig);
export const auth = getAuth(app);
