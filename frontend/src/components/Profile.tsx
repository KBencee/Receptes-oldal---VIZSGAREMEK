import { useContext } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { AuthUserContext } from '../context/AuthenticatedUserContextProvider'
import styles from '../css/Profile.module.css'
import { useQuery } from '@tanstack/react-query'
import { BASE_URL } from '../services/publicAPI'

const API_BASE_URL = BASE_URL + "/api";

const Profile = () => {
    const authUser = useContext(AuthUserContext)
    const navigate = useNavigate()

	const logout = () => {
		localStorage.clear()
		navigate("/")
		window.location.reload()
	} 

	const { data: user } = useQuery({
		queryKey: ["me"],
		queryFn: async () => {
			const res = await fetch(API_BASE_URL + "/api/Auth/me", {
				headers: {
					Authorization: `Bearer ${localStorage.getItem("access")}`,
				},
			});
			if (!res.ok) throw new Error("Failed to fetch user");
			return res.json();
		},
	});

  return (
    <div className={styles.profile}>
		{authUser && <button className={styles.logoutBtn} onClick={logout}><i className="fa-solid fa-arrow-right-from-bracket"></i></button>}
        {!authUser ? 
        	<Link to="/login" className={styles.loginBtn}>Bejelentkezés</Link> 
			: 
			<img src={user?.profileImageUrl ? 
				user.profileImageUrl 
				: 
				"profile.webp"} 
				alt={user?.username} title={user?.username}
			/>}
    </div>
  )
}

export default Profile