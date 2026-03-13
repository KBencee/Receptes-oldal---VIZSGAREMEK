import styles from '../css/ToggleBtn.module.css'

const DescriptionCommentToggleBtn = ({description, setDescription}: {description: boolean, setDescription: (description: boolean) => void}) => {

  return (
    <div className={styles.container}>
        <i className="fa-regular fa-newspaper fa-xl" onClick={() => setDescription(true)}></i>
        <button onClick={() => setDescription(!description)}>
            {description ? <i className="fa-solid fa-toggle-off fa-2xl"></i> : <i className="fa-solid fa-toggle-off fa-flip-horizontal fa-2xl"></i>}
        </button>
        <i className="fa-regular fa-comment fa-xl" onClick={() => setDescription(false)}></i>
    </div>
  )
}

export default DescriptionCommentToggleBtn