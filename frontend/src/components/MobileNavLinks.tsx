import { Link } from 'react-router-dom'
import { menuItems, setActive } from './NavMiddle'
import styles from '../css/Nav.module.css'

const MobileNavLinks = () => {
  return (
    <>
      {menuItems.map(x => <Link to={x.link} onClick={() => setActive(x.name)} className={x.active ? styles.active : ""} key={x.id}>{x.name}</Link>)}
    </>
  )
}

export default MobileNavLinks