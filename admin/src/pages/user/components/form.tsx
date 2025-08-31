import { apiUserCreate } from "@/services/user";
import { apiTeamOptions } from "@/services/users/team";
import { DrawerForm, DrawerFormProps, ProFormDatePicker, ProFormInstance, ProFormSelect, ProFormText } from "@ant-design/pro-components"
import { Col, message, Row } from "antd";
import { useRef } from "react";

type Props = DrawerFormProps & {
    reload?: () => void;
    data?: any;
}

const UserForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>();

    const handleFinish = async (values: any) => {
        if (props.data) {
            // Update user
        } else {
            await apiUserCreate(values);
        }
        message.success("Thao tác thành công");
        formRef.current?.resetFields();
        props.reload?.();
        return true;
    }

    return (
        <DrawerForm title="Tài khoản" {...props} formRef={formRef} onFinish={handleFinish}>
            <Row gutter={[16, 16]}>
                <Col xs={24} md={12}>
                    <ProFormText name="username" label="Tên đăng nhập"
                        rules={[{ required: true, message: "Tên đăng nhập là bắt buộc" }]}
                    />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormText name="name" label="Họ và tên" rules={[{ required: true, message: "Họ và tên là bắt buộc" }]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`gender`} label="Giới tính" options={[
                        { label: "Nam", value: false },
                        { label: "Nữ", value: true },
                    ]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormDatePicker name="dateOfBirth" label="Ngày sinh" fieldProps={{ style: { width: '100%' } }} />
                </Col>
            </Row>
            <ProFormSelect name={`teamId`} label="Team" request={apiTeamOptions} rules={[
                {
                    required: true
                }
            ]} showSearch />
            <div className="md:flex gap-4">
                <div className="flex-1">
                    <ProFormText name="email" label="Email" rules={[
                        {
                            type: "email",
                            message: "Email không hợp lệ"
                        }
                    ]} />
                </div>
                <div className="flex-1">
                    <ProFormText name={`phoneNumber`} label="Số điện thoại" rules={[
                        {
                            required: true,
                            message: "Số điện thoại là bắt buộc"
                        },
                        {
                            pattern: /^\d{10}$/,
                            message: "Số điện thoại không hợp lệ"
                        }
                    ]} />
                </div>
            </div>
            <ProFormText.Password name="password" label="Mật khẩu" rules={[{ required: true, message: "Mật khẩu là bắt buộc" }]} />
        </DrawerForm>
    )
}

export default UserForm;