import { createBrowserRouter } from "react-router-dom";
import Home from "../pages/home";
import Login from "../pages/login";
import Register from "../pages/register";
import Courses from "../pages/courses";
import CourseDetail from "../pages/courseDetail";

export const router = createBrowserRouter([
    { path: "/", element: <Home />},
    { path: "/login", element: <Login /> },
    { path: "/register", element: <Register /> },
    { path: "/courses", element: <Courses /> },
    { path: "/courses/:id", element: <CourseDetail /> }
]);