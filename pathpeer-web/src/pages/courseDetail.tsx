import { useQuery } from '@tanstack/react-query'
import { useNavigate, useParams } from 'react-router-dom'
import client from '../shared/api/axiosClient'
import type { Course } from '../shared/types'

const CourseDetail = () => {
    const { id } = useParams()
    const navigate = useNavigate()

    const { data, isLoading, isError } = useQuery<Course>({
        queryKey: ['course', id],
        queryFn: () => client.get(`/courses/${id}`).then(r => r.data)
    })

    if (isLoading) return <div>Se încarcă...</div>
    if (isError) return <div>Cursul nu a fost găsit</div>

    return (
        <div style={{ maxWidth: 800, margin: '50px auto', padding: 20 }}>
            <button onClick={() => navigate('/courses')}>← Înapoi</button>

            <h1>{data?.title}</h1>
            <p>{data?.description}</p>

            <div style={{ marginTop: 20 }}>
                <p>💰 Preț: {data?.price} lei</p>
                <p>📌 Status: {data?.status}</p>
                <p>👤 Creator: {data?.creatorUsername}</p>
                <p>🌐 Limbă: {data?.language}</p>
                <p>📊 Nivel: {data?.level}</p>
                <p>👍 Voturi: {data?.votesUp} UP / {data?.votesDown} DOWN</p>
                <p>📅 Creat: {new Date(data?.createdAt!).toLocaleDateString()}</p>
            </div>
        </div>
    )
}

export default CourseDetail