import { createBrowserRouter } from "react-router-dom";
import Home from "../pages/home";
import Login from "../pages/login";
import Register from "../pages/register";
import CourseEditorPage from "../pages/courseEditorPage";
import CourseViewPage from "../pages/courseViewPage";
import CoursesPage from "../pages/coursesPage";

export const router = createBrowserRouter([
    { path: "/", element: <Home />},
    { path: "/login", element: <Login /> },
    { path: "/register", element: <Register /> },
    { path: "/courses", element: <CoursesPage /> },
    { path: "/courses/:id", element: <CourseViewPage /> },
    { path: "/courses/create", element: <CourseEditorPage /> },
    { path: "/courses/:id/edit", element: <CourseEditorPage /> }
]);