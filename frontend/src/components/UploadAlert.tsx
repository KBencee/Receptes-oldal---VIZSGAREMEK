
const UploadAlert = (props: { text: string, onClick: () => void }) => {

  return (
    <div id="UploadAlertBox">
      <p>{props.text}</p>
      <input type="button" value="Tovább" onClick={props.onClick} />
    </div>
  );
};

export default UploadAlert;
