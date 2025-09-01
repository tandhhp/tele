import { ModalForm, ModalFormProps, ProFormText } from "@ant-design/pro-components"

type Props = ModalFormProps & {
    data?: any;
    reload?: () => void;
}

const RoomForm: React.FC<Props> = (props) => {
    return (
        <ModalForm
            {...props} title="Thông tin phòng"
        >
            <ProFormText name={`name`} label="Tên phòng" rules={[{ required: true, message: 'Tên phòng là bắt buộc' }]} />
        </ModalForm>
    )
}

export default RoomForm;