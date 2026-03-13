import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import styles from '../css/Headbar.module.css'

const HeadBar = () => {
  return (
    <div className={styles.headBar}>
        <h1>Receptek</h1>
        <p>Ha nincs ötlet a főzéshez...</p>
        <button>Keresés...<FontAwesomeIcon icon={faMagnifyingGlass} /></button>
    </div>
  )
}

export default HeadBar