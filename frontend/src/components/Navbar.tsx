import NavMiddle from './NavMiddle'
import styles from '../css/Nav.module.css'
import Profile from './Profile'
import { useContext } from 'react'
import { AuthUserContext } from '../context/AuthenticatedUserContextProvider'

const Navbar = () => {
    const authUser = useContext(AuthUserContext)

  return (
    <div className={styles.navbar} >
        <div><img src="my-recipes-logo.png" alt="logo" className={styles.logo}/></div>
        {authUser && <NavMiddle/>}
        <Profile/>
    </div>
  )
}

export default Navbar