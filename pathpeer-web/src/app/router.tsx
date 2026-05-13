import { createBrowserRouter } from "react-router-dom";
import Home from "../pages/home";
import Login from "../pages/login";
import Register from "../pages/register";
import CourseEditorPage from "../pages/courseEditorPage";
import CourseViewPage from "../pages/courseViewPage";
import CoursesPage from "../pages/coursesPage";
import ProtectedRoute from "../shared/components/ProtectedRoute";
import GuestRoute from "../shared/components/GuestRoute";

export const router = createBrowserRouter([
    // Publică
    { path: "/", element: <Home /> },

    // Guest only — dacă ești logat te trimite la /
    { path: "/login",    element: <GuestRoute><Login /></GuestRoute> },
    { path: "/register", element: <GuestRoute><Register /></GuestRoute> },

    // Protected — dacă nu ești logat te trimite la /login
    { path: "/courses",             element: <ProtectedRoute><CoursesPage /></ProtectedRoute> },
    { path: "/courses/create",      element: <ProtectedRoute><CourseEditorPage /></ProtectedRoute> },
    { path: "/courses/:id",         element: <ProtectedRoute><CourseViewPage /></ProtectedRoute> },
    { path: "/courses/:id/edit",    element: <ProtectedRoute><CourseEditorPage /></ProtectedRoute> },
]);

// export const router = createBrowserRouter([
//     // Publice
//     { path: "/", element: <Home /> },
//     { path: "/login",    element: <GuestRoute><Login /></GuestRoute> },
//     { path: "/register", element: <GuestRoute><Register /></GuestRoute> },

//     // Orice user logat
//     { path: "/courses",
//       element: <ProtectedRoute><CoursesPage /></ProtectedRoute> },

//     // Teacher + Admin
//     { path: "/courses/create",
//       element: <RoleProtectedRoute allowedRoles={['Teacher', 'Admin']}>
//                    <CourseEditorPage />
//                </RoleProtectedRoute> },

//     // Doar Admin
//     { path: "/admin",
//       element: <RoleProtectedRoute allowedRoles={['Admin']}>
//                    <AdminPage />
//                </RoleProtectedRoute> },
//     { path: "/admin/users",
//       element: <RoleProtectedRoute allowedRoles={['Admin']}>
//                    <AdminUsersPage />
//                </RoleProtectedRoute> },
// ])