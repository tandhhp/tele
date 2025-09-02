import { apiRoomCreate, apiRoomUpdate } from "@/services/event/room";
import { apiBranchOptions } from "@/services/settings/branch";
import { ModalForm, ModalFormProps, ProFormInstance, ProFormSelect, ProFormText } from "@ant-design/pro-components"
import { message } from "antd";
import { useRef } from "react";

type Props = ModalFormProps & {
    data?: any;
    reload?: () => void;
}

const RoomForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>(null);

    const onFinish = async (values: any) => {
        if (props.data) {
            values.id = props.data.id;
            await apiRoomUpdate(values);
        } else {
            await apiRoomCreate(values);
        }
        message.success('Lưu thành công');
        props.reload?.();
        formRef.current?.resetFields();
        return true;
    }

    return (
        <ModalForm
            {...props} title="Thông tin phòng" formRef={formRef} onFinish={onFinish}
        >
            <ProFormSelect name={`branchId`} label="Chi nhánh" request={apiBranchOptions} rules={[
                {
                    required: true, message: 'Chi nhánh là bắt buộc'
                }
            ]} />
            <ProFormText name={`name`} label="Tên phòng" rules={[{ required: true, message: 'Tên phòng là bắt buộc' }]} />
        </ModalForm>
    )
}

export default RoomForm;