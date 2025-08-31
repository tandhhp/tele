import { apiUserOptions } from "@/services/user";
import { apiTeamAddUser } from "@/services/users/team";
import { UserAddOutlined } from "@ant-design/icons";
import { ModalForm, ModalFormProps, ProFormSelect } from "@ant-design/pro-components";
import { useParams } from "@umijs/max";
import { Button, message } from "antd";

type Props = {
    reload?: () => void;
}

const TeamAddUserForm: React.FC<Props> = (props) => {

    const { id } = useParams<{ id: string }>();

    const handleFinish = async (values: any) => {
        values.teamId = id;
        await apiTeamAddUser(values);
        message.success("Thao tác thành công");
        props.reload?.();
        return true;
    }

    return (
        <>
            <ModalForm title="Thêm người dùng vào nhóm" width={400}
                onFinish={handleFinish}
                trigger={<Button type="primary" icon={<UserAddOutlined />}>Thêm người dùng</Button>}
            >
                <ProFormSelect name="userId" label="Người dùng" request={apiUserOptions}
                    rules={[{ required: true, message: "Vui lòng chọn người dùng" }]}
                    showSearch
                />
            </ModalForm>
        </>
    )
}

export default TeamAddUserForm;