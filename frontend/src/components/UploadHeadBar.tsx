import styles from '../css/Headbar.module.css'

const UploadHeadBar = () => {
  return (
    <div className={styles.headBar}>
      <h1 className="profileName">
        Új recept feltöltése
      </h1>
    </div>
  );
};

export default UploadHeadBar;
